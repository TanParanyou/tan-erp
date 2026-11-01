//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/finance_person_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'account_mapping_response.g.dart';

/// AccountMappingResponse
///
/// Properties:
/// * [id] 
/// * [kind] 
/// * [debitAccountCode] 
/// * [debitAccountName] 
/// * [creditAccountCode] 
/// * [creditAccountName] 
/// * [updatedBy] 
/// * [updatedAtUtc] 
/// * [rowVersion] 
@BuiltValue()
abstract class AccountMappingResponse implements Built<AccountMappingResponse, AccountMappingResponseBuilder> {
  @BuiltValueField(wireName: r'id')
  String? get id;

  @BuiltValueField(wireName: r'kind')
  String? get kind;

  @BuiltValueField(wireName: r'debitAccountCode')
  String? get debitAccountCode;

  @BuiltValueField(wireName: r'debitAccountName')
  String? get debitAccountName;

  @BuiltValueField(wireName: r'creditAccountCode')
  String? get creditAccountCode;

  @BuiltValueField(wireName: r'creditAccountName')
  String? get creditAccountName;

  @BuiltValueField(wireName: r'updatedBy')
  FinancePersonResponse? get updatedBy;

  @BuiltValueField(wireName: r'updatedAtUtc')
  DateTime? get updatedAtUtc;

  @BuiltValueField(wireName: r'rowVersion')
  String? get rowVersion;

  AccountMappingResponse._();

  factory AccountMappingResponse([void updates(AccountMappingResponseBuilder b)]) = _$AccountMappingResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AccountMappingResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AccountMappingResponse> get serializer => _$AccountMappingResponseSerializer();
}

class _$AccountMappingResponseSerializer implements PrimitiveSerializer<AccountMappingResponse> {
  @override
  final Iterable<Type> types = const [AccountMappingResponse, _$AccountMappingResponse];

  @override
  final String wireName = r'AccountMappingResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AccountMappingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.id != null) {
      yield r'id';
      yield serializers.serialize(
        object.id,
        specifiedType: const FullType(String),
      );
    }
    if (object.kind != null) {
      yield r'kind';
      yield serializers.serialize(
        object.kind,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountCode != null) {
      yield r'debitAccountCode';
      yield serializers.serialize(
        object.debitAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.debitAccountName != null) {
      yield r'debitAccountName';
      yield serializers.serialize(
        object.debitAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountCode != null) {
      yield r'creditAccountCode';
      yield serializers.serialize(
        object.creditAccountCode,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.creditAccountName != null) {
      yield r'creditAccountName';
      yield serializers.serialize(
        object.creditAccountName,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.updatedBy != null) {
      yield r'updatedBy';
      yield serializers.serialize(
        object.updatedBy,
        specifiedType: const FullType(FinancePersonResponse),
      );
    }
    if (object.updatedAtUtc != null) {
      yield r'updatedAtUtc';
      yield serializers.serialize(
        object.updatedAtUtc,
        specifiedType: const FullType(DateTime),
      );
    }
    if (object.rowVersion != null) {
      yield r'rowVersion';
      yield serializers.serialize(
        object.rowVersion,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    AccountMappingResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AccountMappingResponseBuilder result,
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
        case r'kind':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.kind = valueDes;
          break;
        case r'debitAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountCode = valueDes;
          break;
        case r'debitAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.debitAccountName = valueDes;
          break;
        case r'creditAccountCode':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountCode = valueDes;
          break;
        case r'creditAccountName':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.creditAccountName = valueDes;
          break;
        case r'updatedBy':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(FinancePersonResponse),
          ) as FinancePersonResponse?;
          if (valueDes == null) continue;
          result.updatedBy.replace(valueDes);
          break;
        case r'updatedAtUtc':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(DateTime),
          ) as DateTime?;
          if (valueDes == null) continue;
          result.updatedAtUtc = valueDes;
          break;
        case r'rowVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.rowVersion = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AccountMappingResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AccountMappingResponseBuilder();
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


