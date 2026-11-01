//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/purchase_request_line_request.dart';
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/date.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'purchase_request_request.g.dart';

/// PurchaseRequestRequest
///
/// Properties:
/// * [projectId] 
/// * [requiredByDate] 
/// * [note] 
/// * [lines] 
@BuiltValue()
abstract class PurchaseRequestRequest implements Built<PurchaseRequestRequest, PurchaseRequestRequestBuilder> {
  @BuiltValueField(wireName: r'projectId')
  String? get projectId;

  @BuiltValueField(wireName: r'requiredByDate')
  Date? get requiredByDate;

  @BuiltValueField(wireName: r'note')
  String? get note;

  @BuiltValueField(wireName: r'lines')
  BuiltList<PurchaseRequestLineRequest>? get lines;

  PurchaseRequestRequest._();

  factory PurchaseRequestRequest([void updates(PurchaseRequestRequestBuilder b)]) = _$PurchaseRequestRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PurchaseRequestRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PurchaseRequestRequest> get serializer => _$PurchaseRequestRequestSerializer();
}

class _$PurchaseRequestRequestSerializer implements PrimitiveSerializer<PurchaseRequestRequest> {
  @override
  final Iterable<Type> types = const [PurchaseRequestRequest, _$PurchaseRequestRequest];

  @override
  final String wireName = r'PurchaseRequestRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PurchaseRequestRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.projectId != null) {
      yield r'projectId';
      yield serializers.serialize(
        object.projectId,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requiredByDate != null) {
      yield r'requiredByDate';
      yield serializers.serialize(
        object.requiredByDate,
        specifiedType: const FullType.nullable(Date),
      );
    }
    if (object.note != null) {
      yield r'note';
      yield serializers.serialize(
        object.note,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.lines != null) {
      yield r'lines';
      yield serializers.serialize(
        object.lines,
        specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestLineRequest)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PurchaseRequestRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PurchaseRequestRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'projectId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.projectId = valueDes;
          break;
        case r'requiredByDate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(Date),
          ) as Date?;
          if (valueDes == null) continue;
          result.requiredByDate = valueDes;
          break;
        case r'note':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.note = valueDes;
          break;
        case r'lines':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(PurchaseRequestLineRequest)]),
          ) as BuiltList<PurchaseRequestLineRequest>?;
          if (valueDes == null) continue;
          result.lines.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PurchaseRequestRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PurchaseRequestRequestBuilder();
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


