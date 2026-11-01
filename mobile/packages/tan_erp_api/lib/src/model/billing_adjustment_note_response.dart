//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/finance_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'billing_adjustment_note_response.g.dart';

/// BillingAdjustmentNoteResponse
///
/// Properties:
/// * [id] 
/// * [number] 
/// * [noteType] 
/// * [reason] 
/// * [taxBaseDelta] 
/// * [vatRate] 
/// * [vatDelta] 
/// * [totalDelta] 
/// * [status] 
/// * [createdBy] 
/// * [decidedBy] 
/// * [createdAtUtc] 
/// * [decidedAtUtc] 
@BuiltValue()
abstract class BillingAdjustmentNoteResponse implements Built<BillingAdjustmentNoteResponse, BillingAdjustmentNoteResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'number')
  String? get number;

  @BuiltValueField(wireName: r'noteType')
  String? get noteType;

  @BuiltValueField(wireName: r'reason')
  String? get reason;

  @BuiltValueField(wireName: r'taxBaseDelta')
  double? get taxBaseDelta;

  @BuiltValueField(wireName: r'vatRate')
  double? get vatRate;

  @BuiltValueField(wireName: r'vatDelta')
  double? get vatDelta;

  @BuiltValueField(wireName: r'totalDelta')
  double? get totalDelta;

  @BuiltValueField(wireName: r'status')
  String? get status;

  @BuiltValueField(wireName: r'createdBy')
  FinancePersonResponse? get createdBy;

  @BuiltValueField(wireName: r'decidedBy')
  FinancePersonResponse? get decidedBy;

  @BuiltValueField(wireName: r'createdAtUtc')
  DateTime? get createdAtUtc;

  @BuiltValueField(wireName: r'decidedAtUtc')
  DateTime? get decidedAtUtc;

  BillingAdjustmentNoteResponse._();

  factory BillingAdjustmentNoteResponse([void updates(BillingAdjustmentNoteResponseBuilder b)]) = _$BillingAdjustmentNoteResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(BillingAdjustmentNoteResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<BillingAdjustmentNoteResponse> get serializer => _$BillingAdjustmentNoteResponseSerializer();
}

class _$BillingAdjustmentNoteResponseSerializer implements PrimitiveSerializer<BillingAdjustmentNoteResponse> {
  @override
  final Iterable<Type> types = const [BillingAdjustmentNoteResponse, _$BillingAdjustmentNoteResponse];

  @override
  final String wireName = r'BillingAdjustmentNoteResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    BillingAdjustmentNoteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.number != null) {
      yield r'number';
      yield serializers.serialize(
        object.number,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.noteType != null) {
      yield r'noteType';
      yield serializers.serialize(
        object.noteType,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.reason != null) {
      yield r'reason';
      yield serializers.serialize(
        object.reason,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.taxBaseDelta != null) {
      yield r'taxBaseDelta';
      yield serializers.serialize(
        object.taxBaseDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatRate != null) {
      yield r'vatRate';
      yield serializers.serialize(
        object.vatRate,
        specifiedType: const FullType(double),
      );
    }
    if (object.vatDelta != null) {
      yield r'vatDelta';
      yield serializers.serialize(
        object.vatDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.totalDelta != null) {
      yield r'totalDelta';
      yield serializers.serialize(
        object.totalDelta,
        specifiedType: const FullType(double),
      );
    }
    if (object.status != null) {
      yield r'status';
      yield serializers.serialize(
        object.status,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.createdBy != null) {
      yield r'createdBy';
      yield serializers.serialize(
        object.createdBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.decidedBy != null) {
      yield r'decidedBy';
      yield serializers.serialize(
        object.decidedBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.createdAtUtc != null) {
      yield r'createdAtUtc';
      yield serializers.serialize(
        object.createdAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.decidedAtUtc != null) {
      yield r'decidedAtUtc';
      yield serializers.serialize(
        object.decidedAtUtc,
        specifiedType: const FullType.nullable(DateTime),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    BillingAdjustmentNoteResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required BillingAdjustmentNoteResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'id':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.id = valueDes;
          break;
        case r'number':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.number = valueDes;
          break;
        case r'noteType':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.noteType = valueDes;
          break;
        case r'reason':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.reason = valueDes;
          break;
        case r'taxBaseDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.taxBaseDelta = valueDes;
          break;
        case r'vatRate':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatRate = valueDes;
          break;
        case r'vatDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.vatDelta = valueDes;
          break;
        case r'totalDelta':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(double),
          ) as double?;
          if (valueDes == null) continue;
          result.totalDelta = valueDes;
          break;
        case r'status':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.status = valueDes;
          break;
        case r'createdBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.createdBy.replace(valueDes);
          break;
        case r'decidedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.decidedBy.replace(valueDes);
          break;
        case r'createdAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.createdAtUtc = valueDes;
          break;
        case r'decidedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.decidedAtUtc = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  BillingAdjustmentNoteResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = BillingAdjustmentNoteResponseBuilder();
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


