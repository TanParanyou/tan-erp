//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/branch_rate_override_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_rate_override_list_response.g.dart';

/// BranchRateOverrideListResponse
///
/// Properties:
/// * [items] 
@BuiltValue()
abstract class BranchRateOverrideListResponse implements Built<BranchRateOverrideListResponse, BranchRateOverrideListResponseBuilder> {
  @BuiltValueField(wireName: r'items')
  BuiltList<BranchRateOverrideResponse>? get items;

  BranchRateOverrideListResponse._();

  factory BranchRateOverrideListResponse([void updates(BranchRateOverrideListResponseBuilder b)]) = _$BranchRateOverrideListResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchRateOverrideListResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchRateOverrideListResponse> get serializer => _$BranchRateOverrideListResponseSerializer();
}

class _$BranchRateOverrideListResponseSerializer implements PrimitiveSerializer<BranchRateOverrideListResponse> {
  @override
  final Iterable<Type> types = const [BranchRateOverrideListResponse, _$BranchRateOverrideListResponse];

  @override
  final String wireName = r'BranchRateOverrideListResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchRateOverrideListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.items != null) {
      yield r'items';
      yield serializers.serialize(
        object.items,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BranchRateOverrideResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchRateOverrideListResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchRateOverrideListResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'items':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BranchRateOverrideResponse)]),
          ) as BuiltList<BranchRateOverrideResponse>?;
          if (valueDes == null) continue;
          result.items.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchRateOverrideListResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchRateOverrideListResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


