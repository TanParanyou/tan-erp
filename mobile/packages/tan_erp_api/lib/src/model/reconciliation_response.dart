//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_collection/built_collection.dart';
import 'package:tan_erp_api/src/model/reconciliation_row_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'reconciliation_response.g.dart';

/// ReconciliationResponse
///
/// Properties:
/// * [rowCount] 
/// * [inconsistentCount] 
/// * [rows] 
@BuiltValue()
abstract class ReconciliationResponse implements Built<ReconciliationResponse, ReconciliationResponseBuilder> {
  @BuiltValueField(wireName: r'rowCount')
  int? get rowCount;

  @BuiltValueField(wireName: r'inconsistentCount')
  int? get inconsistentCount;

  @BuiltValueField(wireName: r'rows')
  BuiltList<ReconciliationRowResponse>? get rows;

  ReconciliationResponse._();

  factory ReconciliationResponse([void updates(ReconciliationResponseBuilder b)]) = _$ReconciliationResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ReconciliationResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ReconciliationResponse> get serializer => _$ReconciliationResponseSerializer();
}

class _$ReconciliationResponseSerializer implements PrimitiveSerializer<ReconciliationResponse> {
  @override
  final Iterable<Type> types = const [ReconciliationResponse, _$ReconciliationResponse];

  @override
  final String wireName = r'ReconciliationResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ReconciliationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.rowCount != null) {
      yield r'rowCount';
      yield serializers.serialize(
        object.rowCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.inconsistentCount != null) {
      yield r'inconsistentCount';
      yield serializers.serialize(
        object.inconsistentCount,
        specifiedType: const FullType(int),
      );
    }
    if (object.rows != null) {
      yield r'rows';
      yield serializers.serialize(
        object.rows,
        specifiedType: const FullType.nullable(BuiltList, [FullType(ReconciliationRowResponse)]),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ReconciliationResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ReconciliationResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'rowCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.rowCount = valueDes;
          break;
        case r'inconsistentCount':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.inconsistentCount = valueDes;
          break;
        case r'rows':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(BuiltList, [FullType(ReconciliationRowResponse)]),
          ) as BuiltList<ReconciliationRowResponse>?;
          if (valueDes == null) continue;
          result.rows.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ReconciliationResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ReconciliationResponseBuilder();
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


