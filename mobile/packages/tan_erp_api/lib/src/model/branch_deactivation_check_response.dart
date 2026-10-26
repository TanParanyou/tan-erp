//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/branch_blocker_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'branch_deactivation_check_response.g.dart';

/// BranchDeactivationCheckResponse
///
/// Properties:
/// * [canDeactivate] 
/// * [blockers] 
@BuiltValue()
abstract class BranchDeactivationCheckResponse implements Built<BranchDeactivationCheckResponse, BranchDeactivationCheckResponseBuilder> {
  @BuiltValueField(wireName: r'canDeactivate')
  bool? get canDeactivate;

  @BuiltValueField(wireName: r'blockers')
  BuiltList<BranchBlockerResponse>? get blockers;

  BranchDeactivationCheckResponse._();

  factory BranchDeactivationCheckResponse([void updates(BranchDeactivationCheckResponseBuilder b)]) = _$BranchDeactivationCheckResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BranchDeactivationCheckResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BranchDeactivationCheckResponse> get serializer => _$BranchDeactivationCheckResponseSerializer();
}

class _$BranchDeactivationCheckResponseSerializer implements PrimitiveSerializer<BranchDeactivationCheckResponse> {
  @override
  final Iterable<Type> types = const [BranchDeactivationCheckResponse, _$BranchDeactivationCheckResponse];

  @override
  final String wireName = r'BranchDeactivationCheckResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BranchDeactivationCheckResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.canDeactivate != null) {
      yield r'canDeactivate';
      yield serializers.serialize(
        object.canDeactivate,
        specifiedType: const FullType(bool),
      );
    }
    if (object.blockers != null) {
      yield r'blockers';
      yield serializers.serialize(
        object.blockers,
        specifiedType: const FullType.nullable(BuiltList, [FullType(BranchBlockerResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BranchDeactivationCheckResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BranchDeactivationCheckResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'canDeactivate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.canDeactivate = valueDes;
          break;
        case r'blockers':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(BranchBlockerResponse)]),
          ) as BuiltList<BranchBlockerResponse>?;
          if (valueDes == null) continue;
          result.blockers.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BranchDeactivationCheckResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BranchDeactivationCheckResponseBuilder();
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


